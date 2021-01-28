using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.Infrastructure.Extensions
{
    public class StepPagingExtension<T>
    {
        private readonly PagingData _pagingData = new PagingData();

        public StepPagingExtension(int pageSize, int pageIndex, int totalCount, List<T> tableData)
        {
            _pagingData.PageSize = pageSize;
            _pagingData.PageIndex = pageIndex;
            _pagingData.TotalCount = totalCount;
            _pagingData.TableData = tableData;
        }

        public PagingData SetPagingData()
        {
            _pagingData.TotalPages = (int)Math.Ceiling(_pagingData.TotalCount / (double)_pagingData.PageSize);

            if (_pagingData.PageIndex > _pagingData.TotalPages)
            {
                _pagingData.PageIndex = _pagingData.TotalPages;
            }
            else if (_pagingData.PageIndex < 1)
            {
                _pagingData.PageIndex = 1;
            }

            _pagingData.TableData = _pagingData.TableData.Skip((_pagingData.PageIndex - 1) * _pagingData.PageSize).Take(_pagingData.PageSize).ToList();

            return _pagingData;
        }

        public class PagingData
        {
            public int PageSize { get; set; }
            public int PageIndex { get; set; }
            public int TotalPages { get; set; }
            public int TotalCount { get; set; }
            public List<T> TableData { get; set; }
        }
    }
}
