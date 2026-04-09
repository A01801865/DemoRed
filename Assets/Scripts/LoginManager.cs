using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private TextField inputUser;
    private TextField inputPass;
    private Label mensaje;
    private Button boton;
    private string loginURL = "http://localhost:3000/login";

    // Para guardar el userId si el login sale bien
    public static int userIdActual = -1;

    [System.Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public string message;
        public int userId;
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        inputUser = root.Q<TextField>("inputUser");
        inputPass = root.Q<TextField>("inputPass");
        mensaje = root.Q<Label>("lblMensaje");
        boton = root.Q<Button>("btnLogin");

        boton.clicked += OnLoginClicked;
    }

    void OnDestroy()
    {
        if (boton != null)
            boton.clicked -= OnLoginClicked;
    }

    void OnLoginClicked()
    {
        string user = inputUser.value.Trim();
        string pass = inputPass.value;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            mensaje.text = "Campos vacíos";
            return;
        }

        StartCoroutine(EnviarLogin(user, pass));
    }

    IEnumerator EnviarLogin(string user, string pass)
    {
        mensaje.text = "Conectando...";
        boton.SetEnabled(false);

        LoginRequest datos = new LoginRequest
        {
            username = user,
            password = pass
        };

        string json = JsonUtility.ToJson(datos);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(loginURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            boton.SetEnabled(true);

            if (request.result != UnityWebRequest.Result.Success)
            {
                mensaje.text = "Error de conexión: " + request.error;
                yield break;
            }

            string respuestaJson = request.downloadHandler.text;
            Debug.Log("Respuesta del servidor: " + respuestaJson);

            LoginResponse respuesta = JsonUtility.FromJson<LoginResponse>(respuestaJson);

            if (respuesta == null)
            {
                mensaje.text = "Respuesta inválida del servidor";
                yield break;
            }

            if (respuesta.success)
            {
                userIdActual = respuesta.userId;
                mensaje.text = "Login correcto";
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                mensaje.text = respuesta.message;
            }
        }
    }
}