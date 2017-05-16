using System.Collections;
using System.IO;
using System.Web.Mvc;
using LabyrinthMvc.Models;

namespace LabyrinthMvc.Controllers
{
    public class MapController : Controller
    {
        private string fileName = "labirint.txt";

        public ActionResult Index()
        {
            var labModel = new LabyrinthModel
            {
                Map = new int[0, 0],
                Path = new ArrayList()
            };
            return View(labModel);
        }

        [HttpPost]
        [HandleError()]
        public ActionResult Index(LabyrinthModel myLabyrinthModel)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/"), fileName);
                    file.SaveAs(path);
                }
            }

            var l = new Labyrinth.Labyrinth(Server.MapPath("~/Content/"+fileName));
            myLabyrinthModel.Map = (int [,])l.Map.Clone();

            myLabyrinthModel.StartPointX--;
            myLabyrinthModel.StartPointY--;
            myLabyrinthModel.FinishPointX--;
            myLabyrinthModel.FinishPointY--;

            l.SetBlockType(myLabyrinthModel.StartPointX, myLabyrinthModel.StartPointY, 1);
            l.SetBlockType(myLabyrinthModel.FinishPointX, myLabyrinthModel.FinishPointY, 2);
            if (l.Find())
            {
                myLabyrinthModel.Path = l.Path;
                ViewBag.path = "Маршрут: " + l.PathAsText;
            }
            else
            {
                myLabyrinthModel.Path = new ArrayList();
                ViewBag.path = "Маршрут: Путь не найден";
            }
            return View(myLabyrinthModel);
        }
    }
}
