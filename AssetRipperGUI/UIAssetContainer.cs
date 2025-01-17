﻿using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.Parser.Files.SerializedFiles;
using AssetRipper.Core.Parser.Files.SerializedFiles.Parser;
using AssetRipper.Core.Project;
using AssetRipper.Core.Structure.Collections;
using AssetRipper.Library;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AssetRipper.GUI
{
	public class UIAssetContainer : ProjectAssetContainer
	{

		public UIAssetContainer(Ripper ripper) : base(
			ripper.GameStructure.FileCollection.Exporter,
			ripper.Settings,
			new VirtualSerializedFile(ripper.GameStructure.FileCollection.Layout),
			ripper.GameStructure.FileCollection.FetchAssets(),
			new Collection<IExportCollection>())
		{
		}
		public override IReadOnlyList<FileIdentifier> Dependencies => new List<FileIdentifier>();

		internal Core.Classes.Object.Object LastAccessedAsset { get; set; }

		public override ISerializedFile File => LastAccessedAsset.File;

		public override TransferInstructionFlags ExportFlags => ExportLayout.Info.Flags;
	}
	
}