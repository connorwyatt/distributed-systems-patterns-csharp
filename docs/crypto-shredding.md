# Crypto Shredding

This pattern is to deal with sensitive or personal data in an event sourced system.

The approach here is to encrypt the data in the events before you save them, and then if the data needs to be deleted, you "shred" the key that you used to encrypt the data.
