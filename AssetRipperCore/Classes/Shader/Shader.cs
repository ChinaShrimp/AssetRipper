using AssetRipper.Core.Converters.Shader;
using AssetRipper.Core.Project;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Classes.Material;
using AssetRipper.Core.Classes.Misc;
using AssetRipper.Core.Classes.Shader.Blob;
using AssetRipper.Core.Classes.Shader.Enums;
using AssetRipper.Core.IO;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityVersion = AssetRipper.Core.Parser.Files.UnityVersion;

namespace AssetRipper.Core.Classes.Shader
{
	public sealed class Shader : TextAsset
	{
		public Shader(AssetInfo assetInfo) : base(assetInfo) { }

		public static int ToSerializedVersion(UnityVersion version)
		{
			// double blob arrays (offsets, compressedLengths and decompressedLengths)
			if (version.IsGreaterEqual(2019, 3))
			{
				return 2;
			}
			return 1;
		}

		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public static bool IsSerialized(UnityVersion version) => version.IsGreaterEqual(5, 5);
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public static bool HasBlob(UnityVersion version) => version.IsGreaterEqual(5, 3);
		/// <summary>
		/// Less than 2.0.0
		/// </summary>
		public static bool HasFallback(UnityVersion version) => version.IsLess(2);
		/// <summary>
		/// Less than 3.2.0
		/// </summary>
		public static bool HasDefaultProperties(UnityVersion version) => version.IsLess(3, 2);
		/// <summary>
		/// 2.0.0 to 3.0.0 exclusive
		/// </summary>
		public static bool HasStaticProperties(UnityVersion version) => version.IsGreaterEqual(2) && version.IsLess(3);
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool HasDependencies(UnityVersion version) => version.IsGreaterEqual(4);
		/// <summary>
		/// 2018.1 and greater
		/// </summary>
		public static bool HasNonModifiableTextures(UnityVersion version) => version.IsGreaterEqual(2018);
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool HasShaderIsBaked(UnityVersion version) => version.IsGreaterEqual(4);
		/// <summary>
		/// 3.4.0 to 5.5.0 exclusive and Not Release
		/// </summary>
		public static bool HasErrors(UnityVersion version, TransferInstructionFlags flags)
		{
			return !flags.IsRelease() && version.IsGreaterEqual(3, 4) && version.IsLess(5, 5);
		}
		/// <summary>
		/// 4.2.0 and greater and Not Release
		/// </summary>
		public static bool HasDefaultTextures(UnityVersion version, TransferInstructionFlags flags) => !flags.IsRelease() && version.IsGreaterEqual(4, 2);
		/// <summary>
		/// 4.5.0 and greater and Not Release and Not Buildin
		/// </summary>
		public static bool HasCompileInfo(UnityVersion version, TransferInstructionFlags flags)
		{
			return !flags.IsRelease() && !flags.IsBuiltinResources() && version.IsGreaterEqual(4, 5);
		}

		/// <summary>
		/// 2019.3 and greater
		/// </summary>
		private static bool IsDoubleArray(UnityVersion version) => version.IsGreaterEqual(2019, 3);

		public override void Read(AssetReader reader)
		{
			if (IsSerialized(reader.Version))
			{
				ReadNamedObject(reader);

				ParsedForm.Read(reader);
				Platforms = reader.ReadArray((t) => (GPUPlatform)t);
				if (IsDoubleArray(reader.Version))
				{
					uint[][] offsets = reader.ReadUInt32ArrayArray();
					uint[][] compressedLengths = reader.ReadUInt32ArrayArray();
					uint[][] decompressedLengths = reader.ReadUInt32ArrayArray();
					byte[] compressedBlob = reader.ReadByteArray();
					reader.AlignStream();

					UnpackSubProgramBlobs(reader.Layout(), offsets, compressedLengths, decompressedLengths, compressedBlob);
				}
				else
				{
					uint[] offsets = reader.ReadUInt32Array();
					uint[] compressedLengths = reader.ReadUInt32Array();
					uint[] decompressedLengths = reader.ReadUInt32Array();
					byte[] compressedBlob = reader.ReadByteArray();
					reader.AlignStream();

					UnpackSubProgramBlobs(reader.Layout(), offsets, compressedLengths, decompressedLengths, compressedBlob);
				}
			}
			else
			{
				base.Read(reader);

				if (HasBlob(reader.Version))
				{
					uint decompressedSize = reader.ReadUInt32();
					byte[] compressedBlob = reader.ReadByteArray();
					reader.AlignStream();

					UnpackSubProgramBlobs(reader.Layout(), 0, (uint)compressedBlob.Length, decompressedSize, compressedBlob);
				}

				if (HasFallback(reader.Version))
				{
					Fallback.Read(reader);
				}
				if (HasDefaultProperties(reader.Version))
				{
					DefaultProperties.Read(reader);
				}
				if (HasStaticProperties(reader.Version))
				{
					StaticProperties.Read(reader);
				}
			}

			if (HasDependencies(reader.Version))
			{
				Dependencies = reader.ReadAssetArray<PPtr<Shader>>();
			}
			if (HasNonModifiableTextures(reader.Version))
			{
				NonModifiableTextures = new Dictionary<string, PPtr<Texture>>();
				NonModifiableTextures.Read(reader);
			}
			if (HasShaderIsBaked(reader.Version))
			{
				ShaderIsBaked = reader.ReadBoolean();
				reader.AlignStream();
			}

#if UNIVERSAL
			if (HasErrors(reader.Version, reader.Flags))
			{
				Errors = reader.ReadAssetArray<ShaderError>();
			}
			if (HasDefaultTextures(reader.Version, reader.Flags))
			{
				DefaultTextures = new Dictionary<string, PPtr<Texture>>();
				DefaultTextures.Read(reader);
			}
			if (HasCompileInfo(reader.Version, reader.Flags))
			{
				CompileInfo.Read(reader);
			}
#endif
		}

		public override void ExportBinary(IExportContainer container, Stream stream) => ExportBinary(container, stream, DefaultShaderExporterInstantiator);
		public void ExportBinary(IExportContainer container, Stream stream, Func<UnityVersion, GPUPlatform, ShaderTextExporter> exporterInstantiator)
		{
			if (IsSerialized(container.Version))
			{
				using (ShaderWriter writer = new ShaderWriter(stream, this, exporterInstantiator))
				{
					ParsedForm.Export(writer);
				}
			}
			else if (HasBlob(container.Version))
			{
				using (ShaderWriter writer = new ShaderWriter(stream, this, exporterInstantiator))
				{
					string header = Encoding.UTF8.GetString(Script);
					if (Blobs.Length == 0)
					{
						writer.Write(header);
					}
					else
					{
						Blobs[0].Export(writer, header);
					}
				}
			}
			else
			{
				base.ExportBinary(container, stream);
			}
		}

		public void ExportDummy(IExportContainer container, Stream stream) => ExportDummy(container, stream, DefaultShaderExporterInstantiator);
		public void ExportDummy(IExportContainer container, Stream stream, Func<UnityVersion, GPUPlatform, ShaderTextExporter> exporterInstantiator)
		{
			DummyShaderTextExporter.ExportShader(this, container, stream, exporterInstantiator);
		}

		public override IEnumerable<PPtr<Object.Object>> FetchDependencies(DependencyContext context)
		{
			foreach (PPtr<Object.Object> asset in base.FetchDependencies(context))
			{
				yield return asset;
			}

			if (HasDependencies(context.Version))
			{
				foreach (PPtr<Object.Object> asset in context.FetchDependencies(Dependencies, DependenciesName))
				{
					yield return asset;
				}
			}
		}

		public static ShaderTextExporter DefaultShaderExporterInstantiator(UnityVersion version, GPUPlatform graphicApi)
		{
			switch (graphicApi)
			{
				case GPUPlatform.unknown:
					return new ShaderTextExporter();

				case GPUPlatform.openGL:
				case GPUPlatform.gles:
				case GPUPlatform.gles3:
				case GPUPlatform.glcore:
					return new ShaderGLESExporter();

				case GPUPlatform.metal:
					return new ShaderMetalExporter();

				default:
					return new ShaderUnknownExporter(graphicApi);
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			throw new NotSupportedException();
		}

		private void UnpackSubProgramBlobs(AssetLayout layout, uint offset, uint compressedLength, uint decompressedLength, byte[] compressedBlob)
		{
			if (compressedBlob.Length == 0)
			{
				Blobs = Array.Empty<ShaderSubProgramBlob>();
			}
			else
			{
				Blobs = new ShaderSubProgramBlob[1];
				using (MemoryStream memStream = new MemoryStream(compressedBlob))
				{
					uint[] offsets = new uint[] { offset };
					uint[] compressedLengths = new uint[] { compressedLength };
					uint[] decompressedLengths = new uint[] { decompressedLength };
					Blobs[0].Read(layout, memStream, offsets, compressedLengths, decompressedLengths);
				}
			}
		}

		private void UnpackSubProgramBlobs(AssetLayout layout, uint[] offsets, uint[] compressedLengths, uint[] decompressedLengths, byte[] compressedBlob)
		{
			Blobs = new ShaderSubProgramBlob[offsets.Length];
			using (MemoryStream memStream = new MemoryStream(compressedBlob))
			{
				for (int i = 0; i < Blobs.Length; i++)
				{
					uint[] blobOffsets = new uint[] { offsets[i] };
					uint[] blobCompressedLengths = new uint[] { compressedLengths[i] };
					uint[] blobDecompressedLengths = new uint[] { decompressedLengths[i] };
					Blobs[i].Read(layout, memStream, blobOffsets, blobCompressedLengths, blobDecompressedLengths);
				}
			}
		}

		private void UnpackSubProgramBlobs(AssetLayout layout, uint[][] offsets, uint[][] compressedLengths, uint[][] decompressedLengths, byte[] compressedBlob)
		{
			Blobs = new ShaderSubProgramBlob[offsets.Length];
			using (MemoryStream memStream = new MemoryStream(compressedBlob))
			{
				for (int i = 0; i < Platforms.Length; i++)
				{
					uint[] blobOffsets = offsets[i];
					uint[] blobCompressedLengths = compressedLengths[i];
					uint[] blobDecompressedLengths = decompressedLengths[i];
					Blobs[i].Read(layout, memStream, blobOffsets, blobCompressedLengths, blobDecompressedLengths);
				}
			}
		}

		public override string ExportExtension => "shader";

		public override string ValidName => IsSerialized(File.Version) ? ParsedForm.Name : base.ValidName;

		public GPUPlatform[] Platforms { get; set; }
		public ShaderSubProgramBlob[] Blobs { get; set; }
		public PPtr<Shader>[] Dependencies { get; set; }
		public Dictionary<string, PPtr<Texture>> NonModifiableTextures { get; set; }
		public bool ShaderIsBaked { get; set; }

#if UNIVERSAL
		/// <summary> Editor Only </summary>
		public ShaderError[] Errors { get; set; }
		/// <summary> Editor Only </summary>
		public Dictionary<string, PPtr<Texture>> DefaultTextures { get; set; }
#endif


		public const string ErrorsName = "errors";
		public const string DependenciesName = "m_Dependencies";

		public SerializedShader.SerializedShader ParsedForm;
		public PPtr<Shader> Fallback;
		public UnityPropertySheet DefaultProperties;
		public UnityPropertySheet StaticProperties;
#if UNIVERSAL
		/// <summary> Editor Only </summary>
		public ShaderCompilationInfo CompileInfo;
#endif
	}
}
