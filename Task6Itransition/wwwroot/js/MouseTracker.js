//window.mouseTracker = {
dotNetHelper: null;
mouseX: 0;
mouseY: 0;
isTracking: false;
lastSentX: -1;
lastSentY: -1;

function startTracking(dotNetHelper) {
    this.dotNetHelper = dotNetHelper;
    if (this.isTracking) return;
    this.isTracking = true;

    window.addEventListener('mousemove', (e) => {
        this.mouseX = e.offsetX;
        this.mouseY = e.offsetY;
    });

    const tick = () => {
        if (!this.isTracking) return;

        if (this.mouseX !== this.lastSentX || this.mouseY !== this.lastSentY) {
            this.lastSentX = this.mouseX;
            this.lastSentY = this.mouseY;

            this.dotNetHelper.invokeMethodAsync('MouseMove', this.mouseX, this.mouseY);
        }

        requestAnimationFrame(tick);
    };

    requestAnimationFrame(tick);
}

function stopTracking() {
    this.isTracking = false;
}
//};