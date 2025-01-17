﻿using AssetRipper.Core.Project;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.Misc
{
	public struct OffsetPtr<T> : IAssetReadable, IYAMLExportable where T : struct, IAssetReadable, IYAMLExportable
	{
		public OffsetPtr(T instance)
		{
			Instance = instance;
		}

		public void Read(AssetReader reader)
		{
			Instance.Read(reader);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(DataName, Instance.ExportYAML(container));
			return node;
		}

		public const string DataName = "data";

		public T Instance;
	}
}
