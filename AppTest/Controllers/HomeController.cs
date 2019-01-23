using AppTest.Models;
using AppTest.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppTest.Controllers
{
    public class HomeController : Controller
    {
        HomeServices _service;

        public HomeController()
        {
            _service = new HomeServices();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard()
        {
            var model = new DashboardVM();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            model.Categories = _service.GetCategories(); //Delay 5 seconds
            model.Posts = _service.GetPosts();//Delay 5 seconds
            model.Tags = _service.GetTags();//Delay 5 seconds

            watch.Stop();
            ViewBag.Time = watch.ElapsedMilliseconds;

            return View(model);
        }

        public async Task<ActionResult> DashboardAsyncA()
        {
            var model = new DashboardVM();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Hasta este punto SYNC - Hasta que se encuentra con el primer await
            model.Categories = await _service.GetCategoriesAsync(); //Delay 5 seconds
            //Await --> Se bloquea 5 seconds
            //Despues de ejecutar GetCategoriesAsync continua
            Debug.WriteLine("then GetCategoriesAsync");

            model.Posts = await _service.GetPostsAsync();//Delay 5 seconds
            //Await --> Se bloquea 5 seconds
            Debug.WriteLine("then GetPostsAsync");

            model.Tags = await _service.GetTagsAsync();//Delay 5 seconds
            //Await --> Se bloquea 5 seconds
            Debug.WriteLine("then GetTagsAsync");
            watch.Stop();
            ViewBag.Time = watch.ElapsedMilliseconds;

            /*
             * Si bien usamos async en este caso lo estamos haciendo sincronicamente dato que hacemos 
             * await _service.GetCategoriesAsync(); --> Mandamos a ejecutar y esperamos al mismo tiempo.
             */

            return View(model);
        }

        public async Task<ActionResult> DashboardAsyncB()
        {
            var model = new DashboardVM();

            Stopwatch watch = new Stopwatch();
            watch.Start();


            //Se comienza a ejecutar GetCategoriesAsync
            var categoriesPromise =  _service.GetCategoriesAsync(); //Delay 5 seconds
            Debug.WriteLine("then GetCategoriesAsync");

            //Se comienza a ejecutar GetPostsAsync
            var postsPromise =  _service.GetPostsAsync();//Delay 5 seconds
            Debug.WriteLine("then GetPostsAsync");

            //Se comienza a ejecutar GetTagsAsync
            var tagsPromise = _service.GetTagsAsync();//Delay 5 seconds
            Debug.WriteLine("then GetTagsAsync");

            //En este punto estan las 3 tareas en paralelo

            //Hasta este punto SYNC - Hasta que se encuentra con el primer await
            //Se bloquea el proceso
            model.Categories = await categoriesPromise;
            model.Posts = await postsPromise;
            model.Tags = await tagsPromise;

            /*
             * Usamos async mandamos a ejecutar la promesa y continuamos.
             * En este caso trabajamos async de verdad dado que mandamos a ejecutar a los services y continuamos el hilo de ejecución
             * hasta el primer await.
             * Cuando se llega a await categoriesPromise; 
             * ya tenemos 3 promesas trabajando en paralelo.
             * Tarda 3 veces menos que DashboardAsyncA y Dashboard
             */

            watch.Stop();
            ViewBag.Time = watch.ElapsedMilliseconds;

            return View(model);
        }
    }
}