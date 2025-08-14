namespace AnalisisGrupal;

public partial class Melissa : ContentPage
{
    private const double TipoDeCambioDolar = 18.5;
    public Melissa()
	{
		InitializeComponent();
	}
    private void OnConvertirClicked(object sender, EventArgs e)
    {

        resultadoLabel.IsVisible = false;
        resultadoLabel.Text = "";


        string pesosText = pesosEntry.Text;


        if (string.IsNullOrWhiteSpace(pesosText))
        {
            DisplayAlert("Error", "Por favor, ingrese una cantidad.", "Aceptar");
            return;
        }

        if (double.TryParse(pesosText, out double pesos))
        {

            double dolares = pesos / TipoDeCambioDolar;

            string resultadoFormateado = $"{dolares:F2}";

            resultadoLabel.Text = $"Con {pesos:F2} pesos, puedes tener ${resultadoFormateado} dólares.";
            resultadoLabel.IsVisible = true;
        }
        else
        {
            DisplayAlert("Error", "Ingrese un número válido.", "Aceptar");
        }
    }
}