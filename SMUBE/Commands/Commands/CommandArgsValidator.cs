﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public interface CommandArgsValidator
    {
        bool Validate(CommandArgs args);
    }
}