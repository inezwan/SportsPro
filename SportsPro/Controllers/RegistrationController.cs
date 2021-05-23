﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportsPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace SportsPro.Controllers
{
    public class RegistrationController : Controller
    {
        //controller starts with a private property named context of the SportsProContext type
        private SportsProContext context { get; set; }
        public RegistrationViewModel viewModel;

        //constructor accepts a SportsProContext Object and assigns it to the context property
        //Allows other methods in this class to easily access the SportsProContext Object
        //Works because of the dependecy injection code in the Startup.cs
        public RegistrationController(SportsProContext ctx)
        {
            context = ctx;
            viewModel = new RegistrationViewModel();
        }

        public ViewResult Index()
        {
            var data = new RegistrationViewModel()
            {
                Customer = new Customer { CustomerID = 1002 }
            };

            IQueryable<Customer> query = context.Customers;

            data.Customers = query.ToList();
            return View(data);
        }

        [HttpPost]
        /*
         * store selected Customer in session state.
         */
        public IActionResult Index(RegistrationViewModel selectedCustomer)
        {
            var session = new MySession(HttpContext.Session);
            var sessionCustomer = session.GetCustomer();
            sessionCustomer = context.Customers.Find(selectedCustomer.Customer.CustomerID);
            session.SetCustomer(sessionCustomer);


            return RedirectToAction("RegProduct", "Registration");
        }

        public IActionResult RegProduct()
        {
            var data = new RegistrationViewModel();
            var session = new MySession(HttpContext.Session);
            var sessionCust = session.GetCustomer();
            ViewBag.products = context.CustProds.Include(cp => cp.Product)
                .Where(c => c.CustomerID == sessionCust.CustomerID)
                .Join(context.Products, cp => cp.ProductID,
                p => p.ProductID, (cp,p) => new { p.Name }).ToList();
                


            //IQueryable<CustProd> query = context.CustProds;
            //query = query.Include(p => p.Product);

            /*
             *filters table by active technician i.e. technician stored in session (selected in dropbox 
             *on index action) and filters on incident dateclosed where dateclosed is not specified.
             */
            //query = query.Where(
            //    c => c.CustomerID == sessionCust.CustomerID);

            //data.Products = query.ToList();
            return View();
        }

        /*var invoices = invoiceList.Join(customerList,
         * i => i.CustomerID, c => c.CustomerID,
         * (i, c) => new { c.Name, i.InvoiceDate, i.InvoiceTotal })
         * .Where(ci => ci.InvoiceTotal > 150) .OrderBy(ci => ci.Name)
         * .ThenBy(ci => ci.InvoiceDate)
         * .Select(ci => new { ci.Name, ci.InvoiceTotal });*/
        
    }
}
