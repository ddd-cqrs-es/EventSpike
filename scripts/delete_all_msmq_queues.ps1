[void] [Reflection.Assembly]::LoadWithPartialName("System.Messaging")

[System.Messaging.MessageQueue]::GetPrivateQueuesByMachine("LOCALHOST") | % {".\" + $_.QueueName} | % {[System.Messaging.MessageQueue]::Delete($_); }

echo "All MSMQ queues deleted"