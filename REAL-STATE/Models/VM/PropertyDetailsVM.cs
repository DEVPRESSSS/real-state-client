namespace REAL_STATE.Models.VM
{
    public class PropertyDetailsVM
    {
        public Property Property { get; set; } = new();
        public Agent? Agent { get; set; }   
    }
}