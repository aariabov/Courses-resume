using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class CreateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
create table public.course
(
    id          uuid not null
        constraint course_pk
            primary key,
    name        text not null,
    url         text not null,
    description text not null
);

alter table public.course
    owner to postgres;

create table public.step
(
    id                uuid not null
        constraint step_pk
            primary key,
    name              text not null,
    description       text not null,
    short_description text not null,
    url               text not null,
    course_id         uuid not null
        constraint step_course_id_fk
            references public.course
);

alter table public.step
    owner to postgres;

create table public.lesson
(
    id      uuid not null
        constraint lesson_pk
            primary key,
    name    text not null,
    url     text not null,
    content text not null,
    step_id uuid not null
        constraint lesson_step_id_fk
            references public.step
);

alter table public.lesson
    owner to postgres;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
