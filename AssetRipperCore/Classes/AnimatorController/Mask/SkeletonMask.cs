using AssetRipper.Core.Project;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;
using System;

namespace AssetRipper.Core.Classes.AnimatorController.Mask
{
	public struct SkeletonMask : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			Data = reader.ReadAssetArray<SkeletonMaskElement>();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			throw new NotSupportedException();
		}

		public SkeletonMaskElement[] Data { get; set; }
	}
}
