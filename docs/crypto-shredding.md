# Crypto Shredding

This pattern is to deal with sensitive or personal data in an event sourced system.

The approach here is to encrypt the data in the events before you save them, and then if the data needs to be deleted (for example due to GDPR), you "shred" the key that you used to encrypt the data.

In this example, the resource ID is used to get the key for decrypting the data when you read from the database, but you could keep multiple keys for a given resource in order to give more fine grained control.

If you cannot decrypt the value because the key has been "shredded", you replace it with a fallback value (`<REDACTED>` in this example).
