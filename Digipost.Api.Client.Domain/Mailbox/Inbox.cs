﻿using System.Collections.Generic;

namespace Digipost.Api.Client.Domain.Mailbox
{
    public class Inbox
    {
        public IEnumerable<InboxDocument> Documents { get; set; }
    }
}