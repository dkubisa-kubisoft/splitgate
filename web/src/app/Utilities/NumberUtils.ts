import { Injectable } from '@angular/core';

/**
 * Utility class for numbers.
 */
 @Injectable({
    providedIn: 'root'
  })
export class NumberUtils 
{
    /**
     * Initializes a new instance of the NumberUtils class.
     */
    constructor() 
    {

    }

    /**
     * Zero-pads the requested number to the specified length.
     * @param num The number to pad.
     * @param size The desired length of the resulting string.
     * @returns The string
     */
    zeroPad(num : number, size: number) : string 
    {
        let str = num.toString();
        while (str.length < size) str = "0" + str;
        return str;
    }

}