// Components/Pages/Hotels/HotelList.razor.js

let observer = null;
let dotNetHelper = null;

export function initializeIntersectionObserver(element, dotNetReference) {
    dotNetHelper = dotNetReference;

    // Clean up existing observer if any
    dispose();

    const options = {
        root: null, // viewport
        rootMargin: '100px', // trigger 100px before reaching the element
        threshold: 0.1
    };

    observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                // Element is visible, trigger load more
                dotNetHelper.invokeMethodAsync('LoadMoreHotels');
            }
        });
    }, options);

    if (element) {
        observer.observe(element);
    }
}

export function dispose() {
    if (observer) {
        observer.disconnect();
        observer = null;
    }
    dotNetHelper = null;
}