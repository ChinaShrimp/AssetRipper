﻿using AssetRipper.Core.Project;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.AvatarMask
{
	public sealed class TransformMaskElement : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			Path = reader.ReadString();
			Weight = reader.ReadSingle();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(PathName, Path);
			node.Add(WeightName, Weight);
			return node;
		}

		public string Path { get; set; }
		public float Weight { get; set; }

		public const string PathName = "m_Path";
		public const string WeightName = "m_Weight";
	}
}
