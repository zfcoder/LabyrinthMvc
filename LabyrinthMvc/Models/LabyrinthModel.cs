using System.Collections;
using System.ComponentModel;

namespace LabyrinthMvc.Models
{
    public class LabyrinthModel
    {
        public int[,] Map { get; set; }

        [DisplayName("Начало (X): ")]
        public int StartPointX { get; set; }

        [DisplayName("Начало (Y): ")]
        public int StartPointY { get; set; }

        [DisplayName("Финиш (X): ")]
        public int FinishPointX { get; set; }

        [DisplayName("Финиш (X): ")]
        public int FinishPointY { get; set; }
        public ArrayList Path { get; set; }
    }
}