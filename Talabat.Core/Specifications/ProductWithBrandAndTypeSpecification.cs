using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndTypeSpecification : BaseSpecification<Product>
    {
        public ProductWithBrandAndTypeSpecification(ProductSpecParams specParams) : base(P =>
         (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search)) &&
        (!specParams.BrandId.HasValue || P.ProductBrandId== specParams.BrandId) &&
        (!specParams.TypeId.HasValue || P.ProductTypeId== specParams.TypeId)
        )
        {
            
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc":
                        AddOrderby(P => P.Price);
                        break;
                    case "priceDesc":
                        AddOrderbyDesc(P => P.Price);
                        break;
                    default:
                        AddOrderby(P => P.Name);
                        break;


                }
            }

            else AddOrderby(P => P.Name);

            ApplyPagination(specParams.PageSize*(specParams.PageIndex-1), specParams.PageSize);

        }

        public ProductWithBrandAndTypeSpecification(int id): base(P => P.Id == id) 
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);
        }

    }
}
