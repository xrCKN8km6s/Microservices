﻿using System;

namespace BFF
{
    public class ErrorDetails
    {
        public string TraceId { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}