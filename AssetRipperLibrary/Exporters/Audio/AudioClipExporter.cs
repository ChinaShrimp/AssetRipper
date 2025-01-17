﻿using AssetRipper.Core.Classes.AudioClip;
using AssetRipper.Core.Configuration;
using AssetRipper.Core.Parser.Files.SerializedFiles;
using AssetRipper.Core.Project;
using AssetRipper.Core.Project.Exporters;
using AssetRipper.Core.Structure.Collections;
using AssetRipper.Core.Utils;
using AssetRipper.Library.Configuration;
using System;
using System.IO;

namespace AssetRipper.Library.Exporters.Audio
{
	public class AudioClipExporter : BinaryAssetExporter
	{
		private AudioExportFormat AudioFormat { get; set; }
		public AudioClipExporter(LibraryConfiguration configuration) => AudioFormat = configuration.AudioExportFormat;

		public override IExportCollection CreateCollection(VirtualSerializedFile virtualFile, Core.Classes.Object.Object asset)
		{
			return new AudioFileExportCollection(this, (AudioClip)asset, AudioFormat);
		}

		public override bool Export(IExportContainer container, Core.Classes.Object.Object asset, string path)
		{
			AudioClip audioClip = (AudioClip)asset;
			byte[] decodedData = AudioClipDecoder.GetDecodedAudioClipData(audioClip);
			if (decodedData == null)
				return false;

			if (AudioFormat == AudioExportFormat.Wav)
				decodedData = AudioConverter.ConvertToWav(decodedData);

			using (Stream fileStream = FileUtils.CreateVirtualFile(path))
			{
				using (BufferedStream stream = new BufferedStream(fileStream))
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						writer.Write(decodedData);
					}
				}
			}
			return true;
		}

		public override void Export(IExportContainer container, Core.Classes.Object.Object asset, string path, Action<IExportContainer, Core.Classes.Object.Object, string> callback)
		{
			Export(container, asset, path);
			callback?.Invoke(container, asset, path);
		}

		public override bool IsHandle(Core.Classes.Object.Object asset, CoreConfiguration options)
		{
			return AudioClipDecoder.LibrariesLoaded && AudioFormat != AudioExportFormat.Native;
		}
	}
}
