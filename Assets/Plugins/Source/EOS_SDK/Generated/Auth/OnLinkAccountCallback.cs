// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Auth
{
	/// <summary>
	/// Function prototype definition for callbacks passed to <see cref="AuthInterface.LinkAccount" />
	/// </summary>
	/// <param name="data">A <see cref="LinkAccountCallbackInfo" /> containing the output information and result</param>
	public delegate void OnLinkAccountCallback(ref LinkAccountCallbackInfo data);

	[System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
	internal delegate void OnLinkAccountCallbackInternal(ref LinkAccountCallbackInfoInternal data);
}