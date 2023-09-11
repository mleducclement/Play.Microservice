using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemsRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _itemsRepository = itemsRepository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            return Ok((await _itemsRepository.GetAllAsync()).Select(item => _mapper.Map<ItemDto>(item)));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemsRepository.GetOneAsync(id);
            return item is null ? NotFound() : _mapper.Map<ItemDto>(item);
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> AddAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemsRepository.AddAsync(item);

            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, _mapper.Map<ItemDto>(item));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _itemsRepository.GetOneAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _itemsRepository.UpdateAsync(existingItem);
            
            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveAsync(Guid id)
        {
            var existingItem = await _itemsRepository.GetOneAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await _itemsRepository.RemoveAsync(id);
            
            await _publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}