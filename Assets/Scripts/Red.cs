using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Red : MonoBehaviour
{
    private Button btnLeer;
    private TextField tfResultado;
    private DropdownField opciones;

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

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        tfResultado = root.Q<TextField>("Resultado");
        opciones = root.Q<DropdownField>("Opciones");

        btnLeer = root.Q<Button>("BotonLeer");

        btnLeer.clicked += LeerTextoPlano;
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

        UnityWebRequest request = UnityWebRequest.Post("https://jsonplaceholder.typicode.com/posts",
                                                        datosJson, "application/json");
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
}
