﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shopping.Model.Entities;
using Shopping.Service;
using Shopping.Service.Commands;
using Shopping.Service.Handlers.Store;
using Shopping.Service.Queries;
using Shopping.Service.Queries.Commerce;

namespace Shopping.Web.Controllers
{
    public class ShopController : BaseController
    {

        private ICommandHandler<SearchProducts> searchProductsService;
        private ICommandHandler<GetProduct> getProductService;
        private ICommandHandler<SearchCitys> searchCitiesService;
        private ICommandHandler<SearchCountrys> searchCountriesService;
        private ICommandHandler<SearchStores> searchStoresService;
        private ICommandHandler<SearchCarts> searchCartService;
        private ICommandHandler<SearchWishlists> searchWishListsService;
        private ICommandHandler<SearchOrders> searchOrdersService;
        private ICommandHandler<AddWishlist> addWishListService;

        public ShopController(ICommandHandler<SearchCountrys> searchCountriesService, ICommandHandler<SearchCitys> searchCitiesService, ICommandHandler<SearchProducts> searchProductsService, ICommandHandler<SearchProductCategories> SearchProductCategoriesService, ICommandHandler<SearchWishlists> searchWishListsService, ICommandHandler<GetCart> getCartService, ICommandHandler<SearchCarts> searchCartService, ICommandHandler<SearchOrders> searchOrdersService, ICommandHandler<AddWishlist> addWishListService, ICommandHandler<SearchStores> searchStoresService) : base(SearchProductCategoriesService, getCartService)
        {

            this.searchProductsService = searchProductsService;
            this.getProductService = getProductService;
            this.searchCitiesService = searchCitiesService;
            this.searchCountriesService = searchCountriesService;
            this.searchStoresService = searchStoresService;
            this.searchCartService = searchCartService;
            this.searchWishListsService = searchWishListsService;
            this.addWishListService = addWishListService;
            this.searchOrdersService = searchOrdersService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Stores(SearchStores searchStores)
        {
            //var stores = await searchStoresService.HandleAsync(searchStores);
            //if(((IEnumerable<Store>)stores.Value).FirstOrDefault(s => s.Name == searchStores.Name) == null)
            //{
            //    ViewBag.Stores = await searchStoresService.HandleAsync(searchStores);
            //}

            //var getStores = new GetStore();
            //var sonuc = getStoreService.HandleAsync(getStores).Result;
            //ViewBag.Stores = sonuc.Value;

            Result result = await searchStoresService.HandleAsync(searchStores);
            return View(result.Value);
        }

        public async Task<IActionResult> Products(SearchProducts searchProducts)
        {
            Result result;
            searchProducts.IsAdvancedSearch = true;
            ViewBag.CategoryId = searchProducts.CategoryId;
            result = await searchProductsService.HandleAsync(searchProducts);
            return View(result.Value);
        }
        public async Task<IActionResult> ProductsList(SearchProducts searchProducts)
        {

            ViewBag.PageSize = searchProducts.PageSize;
            ViewBag.Page = searchProducts.PageNumber;
            searchProducts.IsPagedSearch = true;
            ViewBag.CategoryId = searchProducts.CategoryId;
            Result result = await searchProductsService.HandleAsync(searchProducts);
            ViewBag.PageCount = (double)(Math.Ceiling(((double)result.TotalRecordCount / (double)searchProducts.PageSize)));



            return View(result.Value);
        }
        public async Task<IActionResult> ProductsDetails(GetProduct getProduct)
        {
            Result result = await getProductService.HandleAsync(getProduct);
            return View(result.Value);
        }

        public async Task<IActionResult> AddToCart(AddCartItem addCartItem)
        {
            return View();
        }
        public async Task<IActionResult> GetCart()
        {
            // ilk ürün eklendiğinde kullanıcının sepeti yoksa yeni cart oluşturacak

            var cart = new GetCart();
            cart.UserName = User.Identity.Name;
            var result = await getCartService.HandleAsync(cart);
            return View(result.Value);
        }
        public async Task<IActionResult> Checkout()
        {
            var deliveryAddress = new AddressQuery();
            var searchCities = new SearchCitys();
            var searchCountries = new SearchCountrys();

            Result resultCountry = await searchCountriesService.HandleAsync(searchCountries);
            Result resultCity = await searchCitiesService.HandleAsync(searchCities);

            ViewBag.Cities = new SelectList(resultCity.Value, "Id", "Name", deliveryAddress.CountryId);
            ViewBag.Countries = new SelectList(resultCountry.Value, "Id", "Name", deliveryAddress.CityId);

            return View();
        }
        public async Task<IActionResult> WishList()
        {
            var searchWishlist = new SearchWishlists();

            //Result resultWishList = await searchWishListsService.HandleAsync(searchWishlist);      

            searchWishlist.UserName = "Mehmet";

            Result resultGetWishList = await searchWishListsService.HandleAsync(searchWishlist);

            return View(resultGetWishList.Value);
        }

        public async Task<IActionResult> AddWishList(string productId)
        {
            var addWishList = new AddWishlist();
            addWishList.ProductId = productId;
            addWishList.UserName = User.Identity.Name;
            Result resultAddWishList = await addWishListService.HandleAsync(addWishList);
            return RedirectToAction("WishList");
        }


        public async Task<IActionResult> OrdersTracking()
        {
            //identity eklenmesi gerekiyor 
            var searchOrder = new SearchOrders();
            searchOrder.UserName = User.Identity.Name;
            searchOrder.IsAdvancedSearch = true;
            Result result = await searchOrdersService.HandleAsync(searchOrder);
            return View(result.Value);

            //  searchOrder.IsPagedSearch = true;



        }
    }
}