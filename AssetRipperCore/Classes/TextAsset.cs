using AssetRipper.Core.Project;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Parser.Files;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;
using AssetRipper.Core.YAML.Extensions;
using System.IO;
using System.Text;

namespace AssetRipper.Core.Classes
{
	/// <summary>
	/// Script previously
	/// </summary>
	public class TextAsset : NamedObject
	{
		public TextAsset(AssetInfo assetInfo) : base(assetInfo) { }

		/// <summary>
		/// Less than 2017.1
		/// </summary>
		public static bool HasPath(UnityVersion version) => version.IsLess(2017);

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			Script = reader.ReadByteArray();
			reader.AlignStream();

			if (HasPath(reader.Version))
			{
				PathName = reader.ReadString();
			}
		}

		public override void ExportBinary(IExportContainer container, Stream stream)
		{
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write(Script);
			}
		}

		protected void ReadNamedObject(AssetReader reader)
		{
			base.Read(reader);
		}

		protected YAMLMappingNode ExportBaseYAMLRoot(IExportContainer container)
		{
			return base.ExportYAMLRoot(container);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add(ScriptName, Script.ExportYAML());
			return node;
		}

		public string TextScript => Encoding.UTF8.GetString(Script);

		// NOTE: originaly, it is a string. but, since binary files are serialized as TextAsset, we have to sctore its content as byte array
		public byte[] Script { get; protected set; }
		public string PathName { get; protected set; } = string.Empty;

		public const string ScriptName = "m_Script";
	}
}
