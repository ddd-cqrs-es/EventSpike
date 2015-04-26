﻿using System;
using System.Collections.Generic;

namespace EventSpike.Common
{
    public interface IPublisher
    {
        void Publish(object message);
        void Publish(object message, Action<Dictionary<string, string>> setHeaders);
    }
}