using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAuthenticator : NetworkAuthenticator
{


	#region Messages

	public struct AuthRequestMessage : NetworkMessage
	{
		public bool IsVr;
	}

	public struct AuthResponseMessage : NetworkMessage
	{
		public byte code;
		public string message;
	}

	#endregion

	#region Server

	/// <summary>
	/// Called on server from StartServer to initialize the Authenticator
	/// <para>Server message handlers should be registered in this method.</para>
	/// </summary>
	public override void OnStartServer()
	{
		// register a handler for the authentication request we expect from client
		NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
	}

	/// <summary>
	/// Called on server from StopServer to reset the Authenticator
	/// <para>Server message handlers should be registered in this method.</para>
	/// </summary>
	public override void OnStopServer()
	{
		// unregister the handler for the authentication request
		NetworkServer.UnregisterHandler<AuthRequestMessage>();
	}

	/// <summary>
	/// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
	/// </summary>
	/// <param name="conn">Connection to client.</param>
    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
        base.OnServerAuthenticate(conn);
    }

    /// <summary>
    /// Called on server when the client's AuthRequestMessage arrives
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    /// <param name="msg">The message payload</param>
    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
	{
		AuthResponseMessage authResponseMessage = new AuthResponseMessage
		{
			code = 100,
			message = "Success"
		};

		conn.Send(authResponseMessage);
		conn.authenticationData = msg;
		// Accept the successful authentication
		ServerAccept(conn);

	}

	IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		// Reject the unsuccessful authentication
		ServerReject(conn);
	}

	#endregion

	#region Client

	/// <summary>
	/// Called on client from StartClient to initialize the Authenticator
	/// <para>Client message handlers should be registered in this method.</para>
	/// </summary>
	public override void OnStartClient()
	{
		// register a handler for the authentication response we expect from server
		NetworkClient.RegisterHandler<AuthResponseMessage>((Action<AuthResponseMessage>)OnAuthResponseMessage, false);
	}

	/// <summary>
	/// Called on client from StopClient to reset the Authenticator
	/// <para>Client message handlers should be unregistered in this method.</para>
	/// </summary>
	public override void OnStopClient()
	{
		// unregister the handler for the authentication response
		NetworkClient.UnregisterHandler<AuthResponseMessage>();
	}

	/// <summary>
	/// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
	/// </summary>
	public override void OnClientAuthenticate()
	{
		AuthRequestMessage authRequestMessage = new AuthRequestMessage
		{
			IsVr = Player.IsLocalVR
		};

		NetworkClient.connection.Send(authRequestMessage);
	}

	// Deprecated 2021-04-29
	[Obsolete("Call OnAuthResponseMessage without the NetworkConnection parameter. It always points to NetworkClient.connection anyway.")]
	public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg) => OnAuthResponseMessage(msg);

	/// <summary>
	/// Called on client when the server's AuthResponseMessage arrives
	/// </summary>
	/// <param name="msg">The message payload</param>
	public void OnAuthResponseMessage(AuthResponseMessage msg)
	{
		if (msg.code == 100)
		{
			// Debug.LogFormat(LogType.Log, "Authentication Response: {0}", msg.message);

			// Authentication has been accepted
			ClientAccept();
		}
		else
		{
			Debug.LogError($"Authentication Response: {msg.message}");

			// Authentication has been rejected
			ClientReject();
		}
	}

	#endregion
}