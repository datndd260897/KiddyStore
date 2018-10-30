﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using KiddyAPI.Models;

namespace KiddyAPI.Controllers
{
    public class ToysController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Toys
        public IEnumerable<ToyDTO> GettblToys()
        {
            var list = db.tblToys.Where(toy => toy.isActived == true)
                .Select(toy => new ToyDTO { id = toy.id, name = toy.name,
                                            price = toy.price, image = toy.image,
                                            category = toy.category})
                .ToList();
            return list;
        }

        // GET: api/Toys/5
        [ResponseType(typeof(ToyDTO))]
        public IHttpActionResult GettblToy(int id)
        {
            var toy = db.tblToys.SingleOrDefault(t => t.id == id && t.isActived == true);
            ToyDTO dto = new ToyDTO { id = toy.id, name = toy.name,
                                      price = toy.price, image = toy.image,
                                      description = toy.desciption, category = toy.category};
            if (toy == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        //GET: api/Toys?category=BoardGame
        public IEnumerable<ToyDTO> GetToysByCategory(string category)
        {
            var list = db.tblToys.Where(toy => toy.category == category && toy.isActived == true)
                .Select(toy => new ToyDTO
                {
                    id = toy.id,
                    name = toy.name,
                    price = toy.price,
                    image = toy.image
                }).ToList();
            return list;
        }

        //GET: api/Toys?id=1&related=BoardGame
        public IEnumerable<ToyDTO> getHotToysByCategory(int id, string related)
        {
            var list = db.tblToys.OrderByDescending(toy => toy.id)
                .Where(toy => toy.category == related && toy.isActived == true && toy.id != id)
                .Select(toy => new ToyDTO { id = toy.id, name = toy.name, image = toy.image, price = toy.price, category = toy.category })
                .Take(4).ToList();
            return list;
        }

        // PUT: api/Toys/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PuttblToy(int id, ToyDTO tblToy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tblToy.id)
            {
                return BadRequest();
            }

            db.Entry(tblToy).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblToyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Toys
        [ResponseType(typeof(ToyDTO))]
        public IHttpActionResult PosttblToy(ToyDTO toy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tblToys.Add(new tblToy {name = toy.name, price = toy.price, category = toy.category,
                                        desciption = toy.description, image = toy.image,
                                        createdBy = toy.createdBy, isActived = true, quantity = toy.quantity});
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = toy.id }, toy);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblToyExists(int id)
        {
            return db.tblToys.Count(e => e.id == id) > 0;
        }
    }
}