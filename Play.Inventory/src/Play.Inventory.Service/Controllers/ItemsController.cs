using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using CatalogItem = Play.Inventory.Service.Entities.CatalogItem;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryItemsRepository;
        private readonly IRepository<CatalogItem> _catalogItemsRepository;
        private readonly IMapper _mapper; 
        
        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository, IMapper mapper)
        {
            _inventoryItemsRepository = inventoryItemsRepository;
            _catalogItemsRepository = catalogItemsRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }
            
            var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await _catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);

                var inventoryItemDto = _mapper.Map<InventoryItemDto>(inventoryItem);
                return _mapper.Map(catalogItem, inventoryItemDto);
            });
            
            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(GrantItemDto grantItemDto)
        {
            var inventoryItem = await _inventoryItemsRepository.GetOneAsync(item => item.UserId == grantItemDto.UserId && item.CatalogItemId == grantItemDto.CatalogItemId);

            if (inventoryItem is not null)
            {
                inventoryItem.Quantity += grantItemDto.Quantity;
                await _inventoryItemsRepository.UpdateAsync(inventoryItem);
            }
            else
            {
                inventoryItem = CreateInventoryItem(grantItemDto);

                await _inventoryItemsRepository.AddAsync(inventoryItem);
            }

            return Ok();
        }

        private static InventoryItem CreateInventoryItem(GrantItemDto grantItemDto)
        {
            return new InventoryItem()
            {
                CatalogItemId = grantItemDto.CatalogItemId,
                UserId = grantItemDto.UserId,
                Quantity = grantItemDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };
        }
    }
}