﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace DataBinding.Business
{
  public partial class CustomerList
  {
    private void DataPortal_Fetch(SingleCriteria<CustomerList, string> criteria)
    {
      Fetch(criteria.Value);
    }

    private void Fetch(string nameFilter)
    {
      RaiseListChangedEvents = false;

      Customer c1 = Customer.Load(1, "justin", new DateTime(1980, 2, 3));
      Customer c2 = Customer.Load(1, "sam", new DateTime(1974, 5, 16));
      Customer c3 = Customer.Load(1, "john", new DateTime(1991, 12, 29));
      Customer[] children = new Customer[] { c1, c2, c3 };

      var found = from c in children
                  where string.IsNullOrEmpty(nameFilter) || c.Name == nameFilter
                  select c;

      AddRange(found.ToArray());

      RaiseListChangedEvents = true;
    }
  }
}