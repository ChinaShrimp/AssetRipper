﻿using AssetRipper.Core.Project;
using AssetRipper.Core.Classes.ParticleSystem.Curve;
using AssetRipper.Core.Parser.Files;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.ParticleSystem
{
	public sealed class SizeModule : ParticleSystemModule
	{
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public static bool HasAxes(UnityVersion version) => version.IsGreaterEqual(5, 4);

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			Curve.Read(reader);
			if (HasAxes(reader.Version))
			{
				Y.Read(reader);
				Z.Read(reader);
				SeparateAxes = reader.ReadBoolean();
				reader.AlignStream();
			}
		}

		public override YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = (YAMLMappingNode)base.ExportYAML(container);
			node.Add(CurveName, Curve.ExportYAML(container));
			node.Add(YName, GetExportY(container.Version).ExportYAML(container));
			node.Add(ZName, GetExportZ(container.Version).ExportYAML(container));
			node.Add(SeparateAxesName, SeparateAxes);
			return node;
		}

		private MinMaxCurve GetExportY(UnityVersion version)
		{
			return HasAxes(version) ? Y : new MinMaxCurve(1.0f, 1.0f, 1.0f, 0.0f, 1.0f);
		}
		private MinMaxCurve GetExportZ(UnityVersion version)
		{
			return HasAxes(version) ? Z : new MinMaxCurve(1.0f, 1.0f, 1.0f, 0.0f, 1.0f);
		}

		public bool SeparateAxes { get; set; }

		public const string CurveName = "curve";
		public const string YName = "y";
		public const string ZName = "z";
		public const string SeparateAxesName = "separateAxes";

		public MinMaxCurve Curve;
		public MinMaxCurve Y;
		public MinMaxCurve Z;
	}
}
