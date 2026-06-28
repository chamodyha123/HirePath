DELETE FROM AspNetUserRoles
WHERE UserId IN
(
    SELECT Id
    FROM AspNetUsers
    WHERE Email = 'peshanchamoth759@gmail.com'
);

DELETE FROM AspNetUsers
WHERE Email = 'peshanchamoth759@gmail.com';