const links = document.querySelectorAll('.slide ul li a');
const pages = document.querySelectorAll('.page');

links.forEach(link => {
  link.addEventListener('click', e => {
    e.preventDefault();
    const target = link.getAttribute('data-target');

    pages.forEach(page => {
      page.classList.remove('active');
    });

    const activePage = document.getElementById(target);
    if (activePage) {
      activePage.classList.add('active');
    }
  });
});
