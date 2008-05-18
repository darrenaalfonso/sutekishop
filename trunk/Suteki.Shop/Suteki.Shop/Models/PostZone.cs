﻿using System;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class PostZone : IOrderable, IActivatable
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }
    }
}