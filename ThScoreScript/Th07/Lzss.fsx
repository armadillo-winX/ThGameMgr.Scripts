open System.IO

// Get the leftmost from byte
let getFirstBitFromByte (data: byte) =
    int ((data >>> 7) &&& 1uy)

// Get bits from byte
let getBitsFromByte (count: int) (data: byte) =
    seq{
    if count = 1 then
        yield getFirstBitFromByte data
    else
        for i in 0..(count - 1) do
            let shift = 7 - i
            yield int ((data >>> shift) &&& 1uy)
    }

