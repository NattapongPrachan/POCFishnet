// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
	/// <summary>
	/// Input parameters for the <see cref="LobbySearch.CopySearchResultByIndex" /> function.
	/// </summary>
	public struct LobbySearchCopySearchResultByIndexOptions
	{
		/// <summary>
		/// The index of the lobby to retrieve within the completed search query
		/// <seealso cref="LobbySearch.GetSearchResultCount" />
		/// </summary>
		public uint LobbyIndex { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct LobbySearchCopySearchResultByIndexOptionsInternal : ISettable<LobbySearchCopySearchResultByIndexOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private uint m_LobbyIndex;

		public uint LobbyIndex
		{
			set
			{
				m_LobbyIndex = value;
			}
		}

		public void Set(ref LobbySearchCopySearchResultByIndexOptions other)
		{
			m_ApiVersion = LobbySearch.LobbysearchCopysearchresultbyindexApiLatest;
			LobbyIndex = other.LobbyIndex;
		}

		public void Set(ref LobbySearchCopySearchResultByIndexOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = LobbySearch.LobbysearchCopysearchresultbyindexApiLatest;
				LobbyIndex = other.Value.LobbyIndex;
			}
		}

		public void Dispose()
		{
		}
	}
}