﻿using AssetRipper.Core.Project;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Classes.Misc;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.YAML;
using System.Collections.Generic;

namespace AssetRipper.Core.Classes.TerrainData
{
	public struct TreeDatabase : IAssetReadable, IAssetWritable
	{
		public void Read(AssetReader reader)
		{
			TreeInstances = reader.ReadAssetArray<TreeInstance>();
			TreePrototypes = reader.ReadAssetArray<TreePrototype>();
		}

		public void Write(AssetWriter writer)
		{
			TreeInstances.Write(writer);
			TreePrototypes.Write(writer);
		}

		public void ExportYAML(IExportContainer container, YAMLMappingNode node)
		{
			node.Add(TreeInstancesName, TreeInstances.ExportYAML(container));
			node.Add(TreePrototypesName, TreePrototypes.ExportYAML(container));
		}

		public IEnumerable<PPtr<Object.Object>> FetchDependencies(DependencyContext context)
		{
			foreach (PPtr<Object.Object> asset in context.FetchDependencies(TreePrototypes, TreeInstancesName))
			{
				yield return asset;
			}
		}

		public TreeInstance[] TreeInstances { get; set; }
		public TreePrototype[] TreePrototypes { get; set; }

		public const string TreeInstancesName = "m_TreeInstances";
		public const string TreePrototypesName = "m_TreePrototypes";
	}
}
