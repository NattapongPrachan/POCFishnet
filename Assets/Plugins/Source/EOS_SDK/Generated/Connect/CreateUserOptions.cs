// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
	/// <summary>
	/// Input parameters for the <see cref="ConnectInterface.CreateUser" /> function.
	/// </summary>
	public struct CreateUserOptions
	{
		/// <summary>
		/// Continuance token from previous call to <see cref="ConnectInterface.Login" />
		/// </summary>
		public ContinuanceToken ContinuanceToken { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct CreateUserOptionsInternal : ISettable<CreateUserOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_ContinuanceToken;

		public ContinuanceToken ContinuanceToken
		{
			set
			{
				Helper.Set(value, ref m_ContinuanceToken);
			}
		}

		public void Set(ref CreateUserOptions other)
		{
			m_ApiVersion = ConnectInterface.CreateuserApiLatest;
			ContinuanceToken = other.ContinuanceToken;
		}

		public void Set(ref CreateUserOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = ConnectInterface.CreateuserApiLatest;
				ContinuanceToken = other.Value.ContinuanceToken;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_ContinuanceToken);
		}
	}
}