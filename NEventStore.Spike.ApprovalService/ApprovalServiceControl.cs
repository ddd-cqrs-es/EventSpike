﻿using MassTransit;
using Topshelf;

namespace NEventStore.Spike.ApprovalService
{
    internal class ApprovalServiceControl :
        ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}