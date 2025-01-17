﻿using AssetRipper.Core.Project;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.OcclusionCullingData
{
	public struct SceneObjectIdentifier : IAssetReadable, IYAMLExportable
	{
		public SceneObjectIdentifier(long targetObject, long targetPrefab)
		{
			TargetObject = targetObject;
			TargetPrefab = targetPrefab;
		}

		public void Read(AssetReader reader)
		{
			TargetObject = reader.ReadInt64();
			TargetPrefab = reader.ReadInt64();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(TargetObjectName, TargetObject);
			node.Add(TargetPrefabName, TargetPrefab);
			return node;
		}

		public long TargetObject { get; set; }
		public long TargetPrefab { get; set; }

		public const string TargetObjectName = "targetObject";
		public const string TargetPrefabName = "targetPrefab";
	}
}
