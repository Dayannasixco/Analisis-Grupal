namespace AnalisisGrupal;

public partial class MainPage : ContentPage
{
    private List<CalculoSueldo> historialCalculos = new List<CalculoSueldo>();

    public MainPage()
    {
        InitializeComponent();
        ConfigurarInterfazInicial();
    }

    private void ConfigurarInterfazInicial()
    {
        // Establecer valores por defecto
        pickerTipoTrabajo.SelectedIndex = 0;
        entryPagoPorHora.Text = "15.00";
    }

    private void OnTipoTrabajoChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        var tipoSeleccionado = picker.SelectedItem?.ToString();

        // Actualizar información según el tipo de trabajo
        switch (tipoSeleccionado)
        {
            case "Tiempo Completo":
                lblInfoHorasExtras.Text = "ℹ️ Tiempo completo: 40h normales. Horas extras a 1.5x";
                entryHoras.Placeholder = "Ej: 45";
                break;
            case "Medio Tiempo":
                lblInfoHorasExtras.Text = "ℹ️ Medio tiempo: Hasta 20h. Sin horas extras típicamente";
                entryHoras.Placeholder = "Ej: 20";
                break;
            case "Por Horas":
                lblInfoHorasExtras.Text = "ℹ️ Por horas: Pago directo sin beneficios de tiempo extra";
                entryHoras.Placeholder = "Ej: 15";
                break;
            case "Freelance":
                lblInfoHorasExtras.Text = "ℹ️ Freelance: Tarifa fija por hora trabajada";
                entryHoras.Placeholder = "Ej: 30";
                break;
        }
    }

    private void OnCalcularClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar que todos los campos estén completos
            if (string.IsNullOrWhiteSpace(entryNombre.Text))
            {
                MostrarError("Por favor, ingrese el nombre del trabajador.");
                return;
            }

            if (string.IsNullOrWhiteSpace(entryHoras.Text))
            {
                MostrarError("Por favor, ingrese las horas trabajadas.");
                return;
            }

            if (string.IsNullOrWhiteSpace(entryPagoPorHora.Text))
            {
                MostrarError("Por favor, ingrese el pago por hora.");
                return;
            }

            // Convertir valores
            if (!double.TryParse(entryHoras.Text, out double horasTrabajadas) || horasTrabajadas < 0)
            {
                MostrarError("Las horas trabajadas deben ser un número válido mayor o igual a 0.");
                return;
            }

            if (!double.TryParse(entryPagoPorHora.Text, out double pagoPorHora) || pagoPorHora <= 0)
            {
                MostrarError("El pago por hora debe ser un número válido mayor a 0.");
                return;
            }

            if (horasTrabajadas > 168) // 24 horas * 7 días = 168 horas máximas por semana
            {
                MostrarError("Las horas trabajadas no pueden exceder 168 horas por semana.");
                return;
            }

            // Realizar cálculo
            var calculo = CalcularSueldo(
                entryNombre.Text.Trim(),
                pickerTipoTrabajo.SelectedItem?.ToString() ?? "Tiempo Completo",
                horasTrabajadas,
                pagoPorHora
            );

            // Mostrar resultados
            MostrarResultados(calculo);

            // Agregar al historial
            historialCalculos.Add(calculo);
            ActualizarHistorial();

        }
        catch (Exception ex)
        {
            MostrarError($"Error en el cálculo: {ex.Message}");
        }
    }

    private CalculoSueldo CalcularSueldo(string nombre, string tipoTrabajo, double horas, double pagoPorHora)
    {
        var calculo = new CalculoSueldo
        {
            Nombre = nombre,
            TipoTrabajo = tipoTrabajo,
            HorasTotales = horas,
            PagoPorHora = pagoPorHora,
            FechaCalculo = DateTime.Now
        };

        // Determinar horas normales y extras según el tipo de trabajo
        switch (tipoTrabajo)
        {
            case "Tiempo Completo":
                if (horas <= 40)
                {
                    calculo.HorasNormales = horas;
                    calculo.HorasExtras = 0;
                }
                else
                {
                    calculo.HorasNormales = 40;
                    calculo.HorasExtras = horas - 40;
                }
                break;

            case "Medio Tiempo":
                // Medio tiempo generalmente no tiene horas extras
                calculo.HorasNormales = horas;
                calculo.HorasExtras = 0;
                break;

            case "Por Horas":
            case "Freelance":
            default:
                // Trabajos por horas o freelance sin concepto de horas extras
                calculo.HorasNormales = horas;
                calculo.HorasExtras = 0;
                break;
        }

        // Calcular pagos
        calculo.PagoHorasNormales = calculo.HorasNormales * pagoPorHora;
        calculo.PagoHorasExtras = calculo.HorasExtras * (pagoPorHora * 1.5); // 1.5x para horas extras

        calculo.SueldoTotal = calculo.PagoHorasNormales + calculo.PagoHorasExtras;

        return calculo;
    }

    private void MostrarResultados(CalculoSueldo calculo)
    {
        // Información del trabajador
        lblResultadoNombre.Text = $"👤 {calculo.Nombre}";
        lblResultadoTipo.Text = $"💼 {calculo.TipoTrabajo}";

        // Desglose de horas
        lblHorasNormales.Text = $"• Horas normales: {calculo.HorasNormales:F1}h";

        if (calculo.HorasExtras > 0)
        {
            lblHorasExtras.Text = $"• Horas extras: {calculo.HorasExtras:F1}h (a 1.5x)";
            lblHorasExtras.IsVisible = true;
        }
        else
        {
            lblHorasExtras.Text = "• Sin horas extras";
            lblHorasExtras.IsVisible = true;
        }

        // Desglose de pagos
        lblPagoNormal.Text = $"• Pago normal: ${calculo.PagoHorasNormales:F2}";

        if (calculo.PagoHorasExtras > 0)
        {
            lblPagoExtras.Text = $"• Pago horas extras: ${calculo.PagoHorasExtras:F2}";
            lblPagoExtras.IsVisible = true;
        }
        else
        {
            lblPagoExtras.Text = "• Sin pago de horas extras";
            lblPagoExtras.IsVisible = true;
        }

        // Sueldo total
        lblSueldoTotal.Text = $"💰 SUELDO SEMANAL TOTAL: ${calculo.SueldoTotal:F2}";

        // Mostrar el frame de resultados
        frameResultados.IsVisible = true;

        // Hacer scroll hacia los resultados
        //Device.BeginInvokeOnMainThread(async () =>
        //{
        //    await Task.Delay(100);
        //    await ((ScrollView)Content).ScrollToAsync(frameResultados, ScrollToPosition.MakeVisible, true);
        //});
    }

    private void ActualizarHistorial()
    {
        stackHistorial.Children.Clear();

        // Mostrar los últimos 5 cálculos
        var ultimosCalculos = historialCalculos.TakeLast(5).Reverse();

        foreach (var calculo in ultimosCalculos)
        {
            var frame = new Frame
            {
                BackgroundColor = Colors.LightBlue,
                CornerRadius = 5,
                Padding = 10,
                Margin = new Thickness(0, 2)
            };

            var stackLayout = new VerticalStackLayout { Spacing = 2 };

            stackLayout.Children.Add(new Label
            {
                Text = $"👤 {calculo.Nombre} - {calculo.TipoTrabajo}",
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.DarkBlue
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"⏰ {calculo.HorasTotales:F1}h @ ${calculo.PagoPorHora:F2}/h = ${calculo.SueldoTotal:F2}",
                FontSize = 11,
                TextColor = Colors.DarkBlue
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"📅 {calculo.FechaCalculo:dd/MM/yyyy HH:mm}",
                FontSize = 10,
                TextColor = Colors.Gray
            });

            frame.Content = stackLayout;
            stackHistorial.Children.Add(frame);
        }

        frameHistorial.IsVisible = historialCalculos.Count > 0;
    }

    private async void MostrarError(string mensaje)
    {
        await DisplayAlert("❌ Error", mensaje, "OK");
    }

    private void OnLimpiarClicked(object sender, EventArgs e)
    {
        // Limpiar campos de entrada
        entryNombre.Text = string.Empty;
        entryHoras.Text = string.Empty;
        entryPagoPorHora.Text = "15.00";
        pickerTipoTrabajo.SelectedIndex = 0;

        // Ocultar resultados
        frameResultados.IsVisible = false;

        // Mostrar confirmación
        DisplayAlert("✅ Limpiado", "Los campos han sido limpiados.", "OK");
    }

    private void OnLimpiarHistorialClicked(object sender, EventArgs e)
    {
        historialCalculos.Clear();
        frameHistorial.IsVisible = false;
        DisplayAlert("✅ Historial Limpiado", "El historial ha sido eliminado.", "OK");
    }
}

// Clase para almacenar los datos del cálculo
public class CalculoSueldo
{
    public string Nombre { get; set; } = string.Empty;
    public string TipoTrabajo { get; set; } = string.Empty;
    public double HorasTotales { get; set; }
    public double HorasNormales { get; set; }
    public double HorasExtras { get; set; }
    public double PagoPorHora { get; set; }
    public double PagoHorasNormales { get; set; }
    public double PagoHorasExtras { get; set; }
    public double SueldoTotal { get; set; }
    public DateTime FechaCalculo { get; set; }
}


