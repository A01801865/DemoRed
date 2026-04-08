using UnityEngine;
using UnityEngine.UIElements;

public class LoginManager : MonoBehaviour
{
    // Referencias a los elementos de la UI
    private TextField inputUser;
    private TextField inputPass;
    private Label mensaje;
    private Button boton;

    void Start()
    {
        // Obtenemos el UI Documen
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Buscamos los elementos por nombre (los que pusiste en UI Builder)
        inputUser = root.Q<TextField>("inputUser"); // Busca el elemento en la UI por su nombre
        inputPass = root.Q<TextField>("inputPass"); // Busca el elemento en la UI por su nombre
        mensaje = root.Q<Label>("lblMensaje"); // Busca el elemento en la UI por su nombre
        boton = root.Q<Button>("btnLogin"); // Busca el elemento en la UI por su nombre

        // Cuando se presiona el botón, ejecuta la función Login
        boton.clicked += Login;
    }

    void Login()
    {
        // Guardamos lo que escribió el usuario
        string user = inputUser.value;
        string pass = inputPass.value;

        // Validación: si están vacíos
        if(user == "" || pass == "")
        {
            mensaje.text = "Campos vacíos";
            return;
        }

        // Aquí aún no mandamos al servidor (eso es persona 2)
        // Solo mostramos en consola

        Debug.Log("Usuario: " + user);
        Debug.Log("Password: " + pass);

        // Simulación de éxito (para probar)
        if(user == "admin" && pass == "123")
        {
            mensaje.text = "Login correcto";
        }
        else
        {
            mensaje.text = "Datos incorrectos";
        }
    }
}