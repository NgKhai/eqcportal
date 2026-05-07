(function () {
    const chartInstances = [];

    function getPointLabel(point) {
        return point.label ?? point.Label ?? '';
    }

    function getPointValue(point) {
        return Number(point.value ?? point.Value ?? 0);
    }

    function cssVar(name, fallback) {
        const value = getComputedStyle(document.documentElement).getPropertyValue(name).trim();
        return value || fallback;
    }

    function themePalette() {
        const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
        return {
            primary: cssVar('--primary', isDark ? '#8AB4F8' : '#4F46E5'),
            secondary: cssVar('--secondary', isDark ? '#78D9EC' : '#0EA5E9'),
            success: cssVar('--success', isDark ? '#81C995' : '#10B981'),
            accent: cssVar('--accent', isDark ? '#FDD663' : '#F59E0B'),
            danger: cssVar('--danger', isDark ? '#F28B82' : '#EF4444'),
            warning: cssVar('--warning', isDark ? '#FCAD70' : '#F97316'),
            text: cssVar('--text-secondary', isDark ? '#BDC1C6' : '#5F6368'),
            muted: cssVar('--text-muted', '#9AA0A6'),
            border: cssVar('--border-light', isDark ? '#3C4043' : '#E8EAED'),
            card: cssVar('--bg-card', isDark ? '#292A2D' : '#FFFFFF')
        };
    }

    function chartColors() {
        const palette = themePalette();
        return [palette.primary, palette.secondary, palette.success, palette.accent, palette.danger, palette.warning];
    }

    function destroyCharts() {
        while (chartInstances.length) {
            const chart = chartInstances.pop();
            chart.destroy();
        }
    }

    function emptyState(canvas, message) {
        const wrapper = canvas.closest('.chart-canvas-wrap');
        if (!wrapper) {
            return;
        }

        wrapper.innerHTML = '<div class="chart-empty"><i class="fa-regular fa-chart-bar" aria-hidden="true"></i><span>' + message + '</span></div>';
    }

    function hasData(points) {
        return Array.isArray(points) && points.some(point => getPointValue(point) > 0);
    }

    function baseOptions() {
        const palette = themePalette();
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    labels: {
                        color: palette.text,
                        boxWidth: 12,
                        boxHeight: 12,
                        usePointStyle: true
                    }
                },
                tooltip: {
                    backgroundColor: palette.card,
                    borderColor: palette.border,
                    borderWidth: 1,
                    titleColor: palette.text,
                    bodyColor: palette.text,
                    padding: 12
                }
            },
            scales: {
                x: {
                    grid: {
                        color: palette.border
                    },
                    ticks: {
                        color: palette.text
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: palette.border
                    },
                    ticks: {
                        precision: 0,
                        color: palette.text
                    }
                }
            }
        };
    }

    function createChart(canvasId, configFactory, emptyMessage) {
        const canvas = document.getElementById(canvasId);
        if (!canvas || typeof Chart === 'undefined') {
            return;
        }

        const config = configFactory(canvas);
        if (!config) {
            emptyState(canvas, emptyMessage);
            return;
        }

        chartInstances.push(new Chart(canvas, config));
    }

    function renderCharts() {
        const data = window.eqcDashboardCharts;
        if (!data) {
            return;
        }

        destroyCharts();
        const colors = chartColors();

        createChart('employeesByDepartmentChart', function () {
            const points = data.employeesByDepartment || [];
            if (!hasData(points)) {
                return null;
            }

            return {
                type: 'bar',
                data: {
                    labels: points.map(getPointLabel),
                    datasets: [{
                        label: 'Nhân viên',
                        data: points.map(getPointValue),
                        backgroundColor: colors[0],
                        borderRadius: 8,
                        maxBarThickness: 44
                    }]
                },
                options: baseOptions()
            };
        }, 'Chưa có dữ liệu phòng ban');

        createChart('employeesByGenderChart', function () {
            const points = data.employeesByGender || [];
            if (!hasData(points)) {
                return null;
            }

            return {
                type: 'pie',
                data: {
                    labels: points.map(getPointLabel),
                    datasets: [{
                        data: points.map(getPointValue),
                        backgroundColor: colors,
                        borderColor: themePalette().card,
                        borderWidth: 2
                    }]
                },
                options: {
                    ...baseOptions(),
                    scales: {}
                }
            };
        }, 'Chưa có dữ liệu giới tính');

        createChart('attendanceByMonthChart', function () {
            const points = data.attendanceByMonth || [];
            if (!Array.isArray(points) || points.length === 0) {
                return null;
            }

            const palette = themePalette();
            return {
                type: 'line',
                data: {
                    labels: points.map(getPointLabel),
                    datasets: [{
                        label: 'Bản ghi chấm công',
                        data: points.map(getPointValue),
                        borderColor: palette.secondary,
                        backgroundColor: palette.secondary + '22',
                        fill: true,
                        pointRadius: 4,
                        pointBackgroundColor: palette.secondary,
                        tension: 0.35
                    }]
                },
                options: baseOptions()
            };
        }, 'Chưa có dữ liệu chấm công');

        createChart('leaveRequestsByStatusChart', function () {
            const points = data.leaveRequestsByStatus || [];
            if (!hasData(points)) {
                return null;
            }

            return {
                type: 'doughnut',
                data: {
                    labels: points.map(getPointLabel),
                    datasets: [{
                        data: points.map(getPointValue),
                        backgroundColor: colors,
                        borderColor: themePalette().card,
                        borderWidth: 2,
                        hoverOffset: 8
                    }]
                },
                options: {
                    ...baseOptions(),
                    cutout: '64%',
                    scales: {}
                }
            };
        }, 'Chưa có dữ liệu nghỉ phép');
    }

    document.addEventListener('DOMContentLoaded', renderCharts);
    window.addEventListener('eqc:themechanged', renderCharts);
})();
