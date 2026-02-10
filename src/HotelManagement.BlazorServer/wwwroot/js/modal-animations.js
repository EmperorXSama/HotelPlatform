window.modalAnimations = {
    slideUp: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) return;

        anime({
            targets: el,
            translateY: ['100%', '0%'],
            easing: 'easeOutCubic',
            duration: 400
        });
    },

    slideDown: function (elementId) {
        return new Promise(resolve => {
            const el = document.getElementById(elementId);
            if (!el) { resolve(); return; }

            anime({
                targets: el,
                translateY: ['0%', '100%'],
                easing: 'easeInCubic',
                duration: 350,
                complete: resolve
            });
        });
    },

    // Bonus: stagger-animate child elements after modal opens
    staggerChildren: function (parentId, childSelector) {
        const parent = document.getElementById(parentId);
        if (!parent) return;

        anime({
            targets: parent.querySelectorAll(childSelector),
            opacity: [0, 1],
            translateY: [20, 0],
            easing: 'easeOutExpo',
            duration: 500,
            delay: anime.stagger(80, { start: 200 })
        });
    }
};