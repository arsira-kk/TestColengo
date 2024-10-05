using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductSyncApp.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductSyncService _syncService;
        private readonly MongoDbContext _dbContext;

        public ProductsController(ProductSyncService syncService, MongoDbContext dbContext)
        {
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // POST: api/products/sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncProducts(int page = 1, int pageSize = 50)
        {
            try
            {
                // เรียกใช้บริการเพื่อดึงข้อมูลสินค้า
                var products = await _syncService.SyncProductsAsync(page, pageSize);
                
                // ตรวจสอบว่ามีสินค้าหรือไม่
                if (products == null || !products.Any())
                {
                    return NotFound("No products found to sync.");
                }

                // บันทึกสินค้าลงในฐานข้อมูล
                await _dbContext.SaveProductsAsync(products);

                return Ok(new { Message = "Products synced successfully", TotalSynced = products.Count });
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาด
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, string sortBy = "Name", string search = "")
        {
            try
            {
                // ดึงข้อมูลผลิตภัณฑ์จาก MongoDB
                var products = await _dbContext.GetProductsAsync(page, pageSize);

                // ตรวจสอบว่ามีสินค้าในฐานข้อมูลหรือไม่
                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                // การค้นหาตามชื่อ
                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // การจัดเรียงข้อมูลตามชื่อหรือวันที่สร้าง
                products = sortBy.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase)
                    ? products.OrderBy(p => p.CreatedDate).ToList()
                    : products.OrderBy(p => p.Name).ToList();

                // ส่งข้อมูลผลิตภัณฑ์กลับในรูปแบบ JSON พร้อมกับข้อมูล paging
                var totalItems = products.Count;
                var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    Products = pagedProducts
                });
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาด
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
