﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public interface ITaskDefinesARun
    {
        TaskRunEntity Run { get; }
    }
}
