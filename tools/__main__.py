import os
import shutil
import webbrowser
import click


current_dir = os.path.dirname(os.path.realpath(__file__))
coverage_folder = 'coverage'
project_folder = 'TI5yncronizer'
project_full_path = os.path.join(current_dir, '../', project_folder)
coverage_full_path = os.path.join(project_full_path, coverage_folder)
report_folder = 'report'
report_full_path = os.path.join(coverage_full_path, report_folder)
report_file_path = os.path.join(report_full_path, 'index.html')


def run_command_line(command: 'str|list[str]'):
    command_str = command \
        if isinstance(command, str) \
        else ' '.join(command)

    os.system(command_str)


@click.group()
def default() -> None:
    pass  # pragma: no cover


@default.command()
def init():
    """Install dependencies"""
    run_command_line(
        'dotnet tool install -g dotnet-reportgenerator-globaltool'
    )


@default.command()
def clean_coverage():
    """Remove coverage files of TI5yncronizer"""
    if os.path.exists(coverage_full_path):
        shutil.rmtree(coverage_full_path)


@default.command()
def clean():
    """Clean solution TI5yncronizer"""
    run_command_line(f'dotnet clean {project_folder}')


@default.command()
def build():
    """Build TI5yncronizer"""
    run_command_line(f'dotnet build {project_folder}')


@default.command('test-back')
def test_backend():
    """Run tests of TI5yncronizer"""
    run_command_line([
        f'dotnet test "{project_folder}"',
        '--verbosity minimal',
        '--collect:"XPlat Code Coverage"',
        f'--results-directory:"{coverage_full_path}"'
    ])


@default.command('coverage-back')
@click.pass_context
def coverage_backend(ctx):
    """Run tests of TI5yncronizer and generate coverage"""
    clean_coverage.invoke(ctx)
    test_backend.invoke(ctx)
    run_command_line([
        'reportgenerator',
        f'-reports:{coverage_full_path}/*/coverage.cobertura.xml',
        f'-targetdir:{report_full_path}',
        '-reporttypes:HTML',
    ])


@default.command('open-coverage-back')
@click.pass_context
def open_coverage_backend(ctx):
    """Open coverage file, and generate if not exists"""
    if not os.path.exists(report_file_path):
        coverage_backend.invoke(ctx)
    webbrowser.open(report_file_path)


if __name__ == '__main__':
    default()
