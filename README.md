# Scaling Assignment

## usage

csc -unsafe -out:Scaling.exe /main:UnsafeImageResizer Scaling.cs Color_To_GS_Conv.cs

mono Scaling.exe <path/to/img> scalingFactor (no. not percentage)

## Original Image - 24bit image of Einstein (grayscale)

<img src="https://github.com/Rashid12Kandah/Training_Assignment_9/blob/main/Einstein.jpeg" alt="24bit image grayscale of Einstein">

> Image Information
>
> > Original Image Size: 186x182
>
> > Original Image Pixel Format: Format24bppRgb

## After Scaling - 8bit image of Einstein (grayscale) (rescaled by a factor of 2)

<img src="https://github.com/Rashid12Kandah/Training_Assignment_9/blob/main/Rescaled_Einstein_2.jpeg" alt="8bit image rescaled grayscale of Einstein">

> Image Information
>
> > Resized Image Size: 372x364
>
> > Resized Image Pixel Format: Format8bppIndexed
>
> > Time Taken = 8ms


## Original Image - 24bit coloured image of husky

<img src="https://github.com/Rashid12Kandah/Training_Assignment_9/blob/main/husky.jpeg" alt="24bit coloured image of husky">

> Image Information
>
> > Original Image Size: 980x1279
> 
> > Original Image Pixel Format: Format24bppRgb

## After Scaling - 8bit grayscale image of husky (rescaled by a factor of 0.5).

<img src="https://github.com/Rashid12Kandah/Training_Assignment_9/blob/main/Rescaled_Husky_0.5.jpeg" alt="8bit rescaled grayscale image of husky"> 

> Image Information
> 
> > Resized Image Size: 490x639
>
> > Resized Image Pixel Format: Format8bppIndexed
> 
> > Time Taken = 43ms

