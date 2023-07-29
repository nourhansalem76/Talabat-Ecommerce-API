using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithFilterationForCountSpecifications : BaseSpecification<Product>
    {
        public ProductWithFilterationForCountSpecifications(ProductSpecParams specParams) : base(P =>
        (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search)) &&
        (!specParams.BrandId.HasValue || P.ProductBrandId == specParams.BrandId) &&
        (!specParams.TypeId.HasValue || P.ProductTypeId == specParams.TypeId)
        )
        {

        }
    }
}
