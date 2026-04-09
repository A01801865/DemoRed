using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Text;

public class Red : MonoBehaviour
{
    private Button btnLeer;
    private Button btnLogout;
    private TextField tfResultado;
    private DropdownField opciones;

    private string logoutURL = "http://localhost:3000/logout";

    //Estructura para guardar el horóscopo
    public struct Horoscopo
    {
        public string date;
        public string sign;
        public string horoscope;
    }

    public struct Publicacion
    {
        public string title;
        public string body;
        public int userId;
    }

    //Estructura para logout
    public struct LogoutData
    {
        public int userId;
    }

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        tfResultado = root.Q<TextField>("Resultado");
        opciones = root.Q<DropdownField>("Opciones");

        btnLeer = root.Q<Button>("BotonLeer");
        btnLogout = root.Q<Button>("btnLogout");

        btnLeer.clicked += LeerTextoPlano;
        btnLogout.clicked += Logout;
    }

    void OnDisable()
    {
        btnLeer.clicked -= LeerTextoPlano;
        btnLogout.clicked -= Logout;
    }

    private void LeerTextoPlano()
    {
        //StartCoroutine( DescargarTextoPlano() ); //inicia la ejecución concurrente
        StartCoroutine( SubirJson() );
        //Regresa de inmediato
    }

    //Subir JSON
    private IEnumerator SubirJson()
    {
        Publicacion p = new Publicacion
        {
            title = "vámonos!",
            body = "Nos vamos en dos semanas...",
            userId = 123
        };

        string datosJson = JsonUtility.ToJson(p);
        print("JSON " + p);

        UnityWebRequest request = new UnityWebRequest(
            "https://jsonplaceholder.typicode.com/posts",
            "POST"
        );

        byte[] bodyRaw = Encoding.UTF8.GetBytes(datosJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        //Después de CIERTO tiempo continúa
        if (request.result == UnityWebRequest.Result.Success)
        {
            string textoPlano = request.downloadHandler.text;
            tfResultado.value = textoPlano;

            //Convertir en JSON
            Horoscopo h = JsonUtility.FromJson<Horoscopo>(textoPlano);
            tfResultado.value = "Fecha: " + h.date +"\n\n"
                                + "Signo: " + h.sign + "\n\n"
                                + "Horóscopo: " + h.horoscope + "\n\n";

        } else
        {
            tfResultado.value = "Error al descargar los datos";
        }

        request.Dispose(); //Libera el objeto
    }

    //Descarga JSON
    private IEnumerator DescargarTextoPlano()
    {
        string signo = opciones.choices[opciones.index];
        UnityWebRequest request = UnityWebRequest.Get("https://api.api-ninjas.com/v1/horoscope?zodiac=" + signo);
        yield return request.SendWebRequest();

        //Después de CIERTO tiempo continúa
        if (request.result == UnityWebRequest.Result.Success)
        {
            string textoPlano = request.downloadHandler.text;
            tfResultado.value = textoPlano;

            //Convertir en JSON
            Horoscopo h = JsonUtility.FromJson<Horoscopo>(textoPlano);
            tfResultado.value = "Fecha: " + h.date +"\n\n"
                                + "Signo: " + h.sign + "\n\n"
                                + "Horóscopo: " + h.horoscope + "\n\n";

        } else
        {
            tfResultado.value = "Error al descargar los datos";
        }

        request.Dispose(); //Libera el objeto
    }

    //Logout
    void Logout()
    {
        StartCoroutine( EnviarLogout() );
    }

    //Enviar JSON logout
    private IEnumerator EnviarLogout()
    {
        LogoutData data = new LogoutData
        {
            userId = LoginManager.userIdActual
        };

        string datosJson = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(logoutURL, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(datosJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        //Después de CIERTO tiempo continúa
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error logout: " + request.error);
        }

        request.Dispose(); //Libera el objeto

        //Limpiar sesión
        LoginManager.userIdActual = -1;

        //Regresar a login
        SceneManager.LoadScene("Login");
    }
}