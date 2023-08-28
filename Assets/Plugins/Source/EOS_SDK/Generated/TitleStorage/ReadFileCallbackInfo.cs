// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.TitleStorage
{
	/// <summary>
	/// Structure containing the result of a read file request
	/// </summary>
	public struct ReadFileCallbackInfo : ICallbackInfo
	{
		/// <summary>
		/// Result code for the operation. <see cref="Result.Success" /> is returned for a successful request, other codes indicate an error
		/// </summary>
		public Result ResultCode { get; set; }

		/// <summary>
		/// Client-specified data passed into the file read request
		/// </summary>
		public object ClientData { get; set; }

		/// <summary>
		/// Product User ID of the local user who initiated this request (optional, will only be present in case it was provided during operation start)
		/// </summary>
		public ProductUserId LocalUserId { get; set; }

		/// <summary>
		/// The filename of the file that has been finished reading
		/// </summary>
		public Utf8String Filename { get; set; }

		public Result? GetResultCode()
		{
			return ResultCode;
		}

		internal void Set(ref ReadFileCallbackInfoInternal other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			Filename = other.Filename;
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct ReadFileCallbackInfoInternal : ICallbackInfoInternal, IGettable<ReadFileCallbackInfo>, ISettable<ReadFileCallbackInfo>, System.IDisposable
	{
		private Result m_ResultCode;
		private System.IntPtr m_ClientData;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_Filename;

		public Result ResultCode
		{
			get
			{
				return m_ResultCode;
			}

			set
			{
				m_ResultCode = value;
			}
		}

		public object ClientData
		{
			get
			{
				object value;
				Helper.Get(m_ClientData, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_ClientData);
			}
		}

		public System.IntPtr ClientDataAddress
		{
			get
			{
				return m_ClientData;
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId value;
				Helper.Get(m_LocalUserId, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_LocalUserId);
			}
		}

		public Utf8String Filename
		{
			get
			{
				Utf8String value;
				Helper.Get(m_Filename, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_Filename);
			}
		}

		public void Set(ref ReadFileCallbackInfo other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			Filename = other.Filename;
		}

		public void Set(ref ReadFileCallbackInfo? other)
		{
			if (other.HasValue)
			{
				ResultCode = other.Value.ResultCode;
				ClientData = other.Value.ClientData;
				LocalUserId = other.Value.LocalUserId;
				Filename = other.Value.Filename;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_ClientData);
			Helper.Dispose(ref m_LocalUserId);
			Helper.Dispose(ref m_Filename);
		}

		public void Get(out ReadFileCallbackInfo output)
		{
			output = new ReadFileCallbackInfo();
			output.Set(ref this);
		}
	}
}