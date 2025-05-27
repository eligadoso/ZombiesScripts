using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System;

public class UGSAuthenticator : MonoBehaviour
{
    public static bool IsReady =>
        UnityServices.State == ServicesInitializationState.Initialized &&
        AuthenticationService.Instance != null &&
        AuthenticationService.Instance.IsSignedIn;



    private static bool _isRunning = false;
    private static Task _initializationTask;

    public static async Task EnsureInitializedAsync()
    {
        if (_isRunning)
        {
            // Si ya se está inicializando, espera hasta que termine
            while (_isRunning)
                await Task.Delay(100);
            return;
        }

        if (IsReady) return;

        _isRunning = true;

        try
        {
            Debug.Log("🟡 Inicializando Unity Services...");
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("🔐 Autenticando anónimamente...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"✅ Autenticado como: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (AuthenticationException ex)
        {
            if (ex.Message.Contains("already signing in"))
            {
                Debug.Log("🕓 Ya se está autenticando. Esperando a que termine...");

                int waited = 0;
                while (!AuthenticationService.Instance.IsSignedIn && waited < 5000)
                {
                    await Task.Delay(200);
                    waited += 200;
                }

                if (!AuthenticationService.Instance.IsSignedIn)
                    Debug.LogError("⚠️ La autenticación no se completó a tiempo.");
            }
            else
            {
                Debug.LogError("❌ Error de autenticación: " + ex.Message);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("❌ Error general al inicializar: " + ex.Message);
        }
        finally
        {
            _isRunning = false;
        }
    }



    private static async Task InternalInitializeAsync()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.Log("🟡 Inicializando Unity Services...");
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                try
                {
                    Debug.Log("🔐 Intentando autenticación anónima...");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"✅ Autenticado como: {AuthenticationService.Instance.PlayerId}");
                }
                catch (AuthenticationException ex) when (ex.Message.Contains("already signing in"))
                {
                    Debug.Log("🕓 Ya se está autenticando. Esperando 200ms...");
                    await Task.Delay(200);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error en autenticación: " + ex.Message);
        }
        finally
        {
            _isRunning = false;
        }
    }

}
