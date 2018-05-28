﻿using CoreApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Services
{
    public class ProductService
    {
        private static readonly ProductService instance = new ProductService();

        public static ProductService Current()
        {
            return instance;
        }

        public List<ProductDto> Products { get; }

        private ProductService()
        {
            Products = new List<ProductDto>
            {
                new ProductDto
                {
                    Id = 1,
                    Name = "牛奶",
                    Price = new decimal(2.5),
                    Description = "描述",
                    Materials=new List<MaterialDto>{
                        new MaterialDto
                        {
                            Id = 1,
                            Name = "水"
                        },
                        new MaterialDto
                        {
                            Id = 2,
                            Name = "奶粉"
                        }
                    }
                },
                new ProductDto
                {
                    Id = 2,
                    Name = "面包",
                    Price = new decimal(4.5),
                    Description = "描述",
                    Materials = new List<MaterialDto>
                    {
                        new MaterialDto
                        {
                            Id = 3,
                            Name = "面粉"
                        },
                        new MaterialDto
                        {
                            Id = 4,
                            Name = "糖"
                        }
                    }
                },
                new ProductDto
                {
                    Id = 3,
                    Name = "啤酒",
                    Price = new decimal(7.5),
                    Description = "描述",
                    Materials = new List<MaterialDto>
                    {
                        new MaterialDto
                        {
                            Id = 5,
                            Name = "麦芽"
                        },
                        new MaterialDto
                        {
                            Id = 6,
                            Name = "地下水"
                        }
                    }
                }
            };
        }
    }
}
