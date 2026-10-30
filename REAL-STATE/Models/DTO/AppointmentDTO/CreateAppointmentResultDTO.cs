namespace REAL_STATE.Models.DTO.AppointmentDTO
{
    public class CreateAppointmentResultDTO
    {
        public bool Success => Appointment != null && Errors.Count == 0;
        public bool PropertyNotFound { get; init; }
        public ViewingAppointment? Appointment { get; init; }
        public string? PropertyTitle { get; init; }

        /// <summary>Key is the model property name, or empty for a general error.</summary>
        public List<(string Key, string Message)> Errors { get; init; } = new();
    }
}
